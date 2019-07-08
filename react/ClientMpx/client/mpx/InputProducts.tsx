import * as React from 'react'
import { Grid, IGridHeader, IGridColumn, _, gridUtil, } from '@ZenComponents/grid/gridExports'

import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputProductsHeaders from './inputProductsHeaders'

import { GridTitleBar } from './GridTitleBar'

const tableName = 'products'
const idColumn = 'prodid'

export class InputProducts extends React.Component<{}, {}> {
  state = {
    showAdvanced: false,
    headers: [] as IGridHeader[],
    data: [],
    selectedRows: [],
  }

  componentWillMount() {
    let headers = inputProductsHeaders.buildHeaders(this.state.showAdvanced)
    this.setState({ headers })
    this._fetch()
  }

  _fetch = async () => {
    let data = await mpxTableUtil.fetchTableJson(tableName)
    // Filter out NONE row
    data = _.filter(data, c => c.proddesc !== 'NONE')
    this.setState({ data })
  }

  _onUpdateCell = (newValue: any, column: IGridColumn, dataRow: any) => {
    console.log('update', newValue, column.header.dataItem, dataRow.id)
    dataRow[column.header.dataItem] = newValue
    this.setState({ data: this.state.data })

    mpxTableUtil.updateTable(
      'products',
      dataRow.prodid,
      column.header.dataItem,
      newValue
    )
  }

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputProductsHeaders.buildHeaders(showAdvanced)
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
          title="Products"
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
