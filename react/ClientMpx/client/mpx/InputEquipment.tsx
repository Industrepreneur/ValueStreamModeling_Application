import * as React from 'react'
import { Json } from '@ZenComponents/common/Json'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@ZenComponents/grid/gridExports'
 
import * as mpxTableUtil from './mpxTableUtil'
import * as inputTableHelper from './inputTableHelper'
import * as inputEquipmentHeaders from './inputEquipmentHeaders'

import { GridTitleBar } from './GridTitleBar'

const tableName = 'equipment'
const idColumn = 'equipid'

export class InputEquipment extends React.Component<{}> {
  state = {
    laborOptions: [],
    data: [],
    selectedRows: [],
    showAdvanced: false,
    headers: [] as IGridHeader[],
  }

  componentWillMount() {
    this._fetch()
  }

  _fetch = async () => {
    let data = await mpxTableUtil.fetchTableJson(tableName)
    // Filter out NONE row
    data = _.filter(data, c => c.equipdesc !== 'NONE')
    let laborOptions = await mpxTableUtil.selectList('labor', 'labordesc')
    let headers = inputEquipmentHeaders.buildHeaders(
      this.state.showAdvanced,
      laborOptions
    )
    this.setState({ data, laborOptions, headers })
  }

  _onUpdateCell = (newValue: any, column: IGridColumn, dataRow: any) => {
    console.log('update', newValue, column.header.dataItem, dataRow.id)
    dataRow[column.header.dataItem] = newValue

    if (column.header.dataItem === 'equiptypename') {
      // Update our Qty (business logic)
      let newQty = 1
      if (newValue === 'Delay') {
        newQty = -1
      } else {
        newQty = 1
      }
      dataRow['grpsiz'] = newQty
      mpxTableUtil.updateTable(tableName, dataRow[idColumn], 'grpsiz', newQty)
    }

    this.setState({ data: this.state.data })

    mpxTableUtil.updateTable(
      tableName,
      dataRow[idColumn],
      column.header.dataItem,
      newValue
    )
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

  _onToggleShowAdvanced = () => {
    console.log('toggle show advanced')
    let showAdvanced = !this.state.showAdvanced
    let headers = inputEquipmentHeaders.buildHeaders(
      showAdvanced,
      this.state.laborOptions
    )
    this.setState({ showAdvanced, headers })
  }



  render() {
    return (
      <div>
        <GridTitleBar
          title="Equipment"
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
        {/* <Json data={this.state.headers} /> */}
        {/*  
        <Json data={this.state.laborOptions} />
        <Json
          data={gridUtil.mapStringsArrayToLabelValueArray(
            this.state.laborOptions
          )}
        />
        <Json data={this.state.data} /> */}
      </div>
    )
  }
}
