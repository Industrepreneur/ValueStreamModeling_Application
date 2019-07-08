import * as mpxTableUtil from './mpxTableUtil'
import { _, gridUtil } from '@ZenComponents/grid/gridExports'

interface ITarget
  extends React.Component<
      any,
      {
        selectedRows: any[]
        data: any[]
      }
    > {
  _fetch: () => any
}

export async function deleteSelectedRows(target: ITarget, tableName, idColumn) {
  console.log('delete', target.state.selectedRows)
  let { selectedRows } = target.state

  let promises = []
  _.forEach(selectedRows, selectedRow => {
    promises.push(mpxTableUtil.fetchDeleteRow(tableName, selectedRow[idColumn]))
  })
  // Wait for all deletes to complete
  await Promise.all(promises)

  // Fetch new data
  await target._fetch()

  // Clear selected rows
  target.setState({
    selectedRows: [],
  })
}

export async function addNewRow(target: ITarget, tableName, idColumn) {
  await mpxTableUtil.fetchAddRow(tableName)
  await target._fetch()
  // Select the new row
  let data = target.state.data
  let newRow = _.maxBy(data, c => c[idColumn])
  target.setState({
    selectedRows: [newRow],
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
