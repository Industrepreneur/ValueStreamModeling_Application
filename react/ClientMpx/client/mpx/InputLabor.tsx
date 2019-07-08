import * as React from 'react'
import { Json } from '@ZenComponents/common/Json'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@ZenComponents/grid/gridExports'

import { GridTitleBar } from './GridTitleBar'

import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputLaborHeaders from './inputLaborHeaders'

const tableName = 'labor'
const idColumn = 'laborid'

export class InputLabor extends React.Component<{}, {}> {
  state = {
    data: [],
    selectedRows: [],
    showAdvanced: false,
    headers: [] as IGridHeader[],
  }

  componentWillMount() {
    this._fetch()
  }

  _fetch = async () => {
    let headers = inputLaborHeaders.buildHeaders(this.state.showAdvanced)
    this.setState({ headers })

    let data = await mpxTableUtil.fetchTableJson(tableName)
    // Filter out NONE row
    data = _.filter(data, c => c.labordesc !== 'NONE')
    this.setState({ data })
  }

  _onUpdateCell = (newValue: any, column: IGridColumn, dataRow: any) => {
    console.log('update', newValue, column.header.dataItem, dataRow.id)
    dataRow[column.header.dataItem] = newValue
    this.setState({ data: this.state.data })
  }

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputLaborHeaders.buildHeaders(showAdvanced)
    this.setState({ showAdvanced, headers })
  }

  _onDeleteSelectedRows = async () => {
    await inputTableHelper.deleteSelectedRows(this, tableName, idColumn)    
  }

  _onAddRow = async () => {
    await inputTableHelper.addNewRow(this, tableName, idColumn)   
  }

  _onSelectRow = (dataRow, isSelected, mode) => {
    inputTableHelper.selectRow(this, dataRow, isSelected, mode)  
  }

  render() {
    return (
      <div>
        <GridTitleBar
          title="Labor"
          showAdvanced={this.state.showAdvanced}
          onToggleShowAdvanced={this._onToggleShowAdvanced}
          onAdd={this._onAddRow}
          onDeleteSelected={this._onDeleteSelectedRows}
          hasSelected={this.state.selectedRows.length > 0}
        />

        <Grid
          headers={this.state.headers}
          data={this.state.data}
          onUpdateCell={this._onUpdateCell}
          selectedRows={this.state.selectedRows}
          onSelectRow={this._onSelectRow}
        />
      </div>
    )
  }
}
