import * as React from 'react'
import { Json } from '@zen-components/common/Json'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

import { GridTitleBar } from './GridTitleBar'
import { DialogError } from './DialogError'

import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputLaborHeaders from './inputLaborHeaders'

const apiName = 'labor'
const tableName = 'labor'
const idColumn = 'laborid'

export class InputLabor extends React.Component<{}, {}> {
  state = {
    data: [],
    selectedRows: [],
    showAdvanced: false,
    headers: [] as IGridHeader[],
    hasError: false,
    error: null as string,
  }

  componentWillMount() {
    this._fetch()
  }

  _fetch = async () => {
    let headers = inputLaborHeaders.buildHeaders(this.state.showAdvanced)
    this.setState({ headers })

    let data = await mpxTableUtil.fetchTableJson(apiName)
    // Filter out NONE row
    data = _.filter(data, c => c.labordesc !== 'NONE')
    this.setState({ data })
  }

  _onUpdateCell = async (newValue: any, column: IGridColumn, dataRow: any) => {
    await inputTableHelper.updateRow(this, apiName, idColumn, dataRow[idColumn], column.header.dataItem, newValue)
  }

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputLaborHeaders.buildHeaders(showAdvanced)
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
         <DialogError isOpen={this.state.hasError} error={this.state.error} onClose={this._onClose_dialogError} />
      </div>
    )
  }
}
