import * as mpxTableUtil from './mpxTableUtil'
import { _, gridUtil } from '@zen-components/grid/gridExports'

interface ITarget
  extends React.Component<
  any,
  {
    selectedRows: any[]
    data: any[]
    error?: string,
    hasError?: boolean,
  }
  > {
  _fetch: () => any
}

// See if a call returned an error
function checkAndSetError(target: ITarget, json): boolean {
  let msg: string = json.d
  if (_.startsWith(msg, 'err|')) {
    console.log('error', msg)
    target.setState({
      error: msg.substring(4),
      hasError: true,
    })
    return true
  }
  return false
}
function getOkResult(target: ITarget, json): string {
  let msg: string = json.d
  if (_.startsWith(msg, 'ok|')) {
    console.log('ok', msg)
    return msg.substring(3)
  }
  return ''
}

export async function updateRow(target: ITarget, apiName, idColumn, id, dataItem, newValue) {
  console.log('update', apiName, idColumn, id, dataItem, newValue)

  let data = target.state.data
  let dataRow = _.find(data, c => c[idColumn] === id)
  let originalValue = dataRow[dataItem]
  dataRow[dataItem] = newValue
  target.setState({ data: data })

  let result = await mpxTableUtil.updateTable(
    apiName,
    id,
    dataItem,
    newValue
  )

  let jsonResult = await result.json()
  if (await checkAndSetError(target, jsonResult)) {
    // Reset value?
    dataRow[dataItem] = originalValue
    target.setState({ data: data })
    return
  }

  // Get result value
  let newValue2 = getOkResult(target, jsonResult)
  // Check to see if server did any formatting
  if (newValue2 !== newValue) {
    dataRow[dataItem] = newValue2
    target.setState({ data: data })
  }


}

export async function addNewRow(target: ITarget, apiName, idColumn, param1?) {
  let result = await mpxTableUtil.fetchAddRow(apiName, param1)
  let jsonResult = await result.json()
  if (await checkAndSetError(target, jsonResult)) {
    return
  }

  await target._fetch()
  // Select the new row
  let data = target.state.data
  let newRow = _.maxBy(data, c => c[idColumn])
  target.setState({
    selectedRows: [newRow],
  })
}

export async function deleteSelectedRows(target: ITarget, apiName, idColumn) {
  console.log('delete', target.state.selectedRows)
  let { selectedRows } = target.state

  let promises = []
  _.forEach(selectedRows, selectedRow => {
    promises.push(mpxTableUtil.fetchDeleteRow(apiName, selectedRow[idColumn]))
  })
  // Wait for all deletes to complete
  let results = await Promise.all(promises)
  _.forEach(results, async (c) => {
    checkAndSetError(target, c.json())
  })

  // Fetch new data
  await target._fetch()

  // Clear selected rows
  target.setState({
    selectedRows: [],
  })
}


export function selectRow(target: ITarget, dataRow, isSelected, mode) {
  target.setState({
    selectedRows: gridUtil.selectRow(
      target.state.selectedRows,
      dataRow,
      isSelected,
      mode
    ),
  })
}
