import * as React from 'react'
import { Grid, IGridHeader, IGridColumn, _, gridUtil, } from '@zen-components/grid/gridExports'
import * as paramsParser from '@zen-components/util/paramsParser'
import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputProductsHeaders from './inputProductsHeaders'

import { InputProductsRoutings } from './InputProductsRoutings'
import { InputProductsIbom } from './InputProductsIbom'
import { InputProductsOperations } from './InputProductsOperations'

import { GridTitleBar } from './GridTitleBar'
import { DialogError } from './DialogError'

const apiName = 'products'
const tableName = 'products'
const idColumn = 'prodid'

export class InputProducts extends React.Component<{}, {}> {
  state = {
    showAdvanced: false,
    headers: [] as IGridHeader[],
    data: [],
    selectedRows: [],
    hasError: false,
    error: null as string,
  }

  componentWillMount() {
    let headers = inputProductsHeaders.buildHeaders(this.state.showAdvanced)
    this.setState({ headers })
    this._fetch()
  }

  _fetch = async () => {
    let data = await mpxTableUtil.fetchTableJson(apiName)
    // Filter out NONE row
    data = _.filter(data, c => c.proddesc !== 'NONE')
    this.setState({ data })
  }

  _onUpdateCell = async (newValue: any, column: IGridColumn, dataRow: any) => {
    await inputTableHelper.updateRow(this, apiName, idColumn, dataRow[idColumn], column.header.dataItem, newValue)
  }

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputProductsHeaders.buildHeaders(showAdvanced)
    this.setState({ showAdvanced, headers })
  }

  _onDeleteSelectedRows = async () => {
    await inputTableHelper.deleteSelectedRows(this, apiName, idColumn)    
  }

  _onAddRow = async () => {
    await inputTableHelper.addNewRow(this, apiName, idColumn)   
  }

  _onSelectRow = (dataRow, isSelected, mode) => {
    inputTableHelper.selectRow(this, dataRow, isSelected, mode)  
  }

  _onClose_dialogError = () => {
    this.setState({ hasError: false })
  }

  render() {

    // Get our mode
    const mode = paramsParser.getUrlParameter('mode')

    if(mode === 'operations') {
      return (<InputProductsOperations />)
    }
    if(mode === 'routings') {
      return (<InputProductsRoutings />)
    }
    if(mode === 'ibom') {
      return (<InputProductsIbom />)
    }

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
         <DialogError isOpen={this.state.hasError} error={this.state.error} onClose={this._onClose_dialogError} />
      </div>
    )
  }
}
