import { React, _ } from './gridImports'
import { IGridStyle } from './IGridStyle'
import { IGridColumn } from './IGridColumn'
import { defaultColumnWidth, defaultColumnHeight } from './gridDefaults'

import { GridPowerCell } from './GridPowerCell'

import { GridCell } from './GridCell'

import { GridCellEditText } from './GridCellEditText'
import { GridCellEditSelect } from './GridCellEditSelect'
import { GridCellEditCheck } from './GridCellEditCheck'
import { GridCellCheck } from './GridCellCheck'
export class GridRowCellPicker extends React.Component<
  {
    column: IGridColumn
    isEditing: boolean
    gridStyle: IGridStyle
    dataRow: any
    isHighlightedRow: boolean
    highlightedColumn: IGridColumn
    onClickCell: (ev, column: IGridColumn, dataRow: any) => void
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void
    onSelectRow?: (dataRow: any, isSelected: boolean, mode: 'single' | 'multiple') => void
    selectedRows: any[],
  },
  {}
> {
  render() {
    let {
      column,
      isEditing,
      gridStyle,
      dataRow,
      isHighlightedRow,
      highlightedColumn,
      selectedRows,
      onSelectRow,
    } = this.props

    // if (column.header.power) {
    //   return (
    //     <GridPowerCell
    //       isEditing={isEditing}
    //       isNew={isNewRow}
    //       column={column}
    //       gridStyle={gridStyle}
    //       dataRow={dataRow}
    //       onCloneRow={this.props.onCloneRow}
    //       onEditRow={this.props.onEditRow}
    //       onDeleteRow={this.props.onDeleteRow}
    //       onAddRow={this.props.onAddRow}
    //     />
    //   )
    // }
    if (column.header.power) {
      return <div>power row currently unsupported</div>
    }
    if (column.header.check) {
      return (
        <div>
          <GridCellCheck
            gridStyle={gridStyle}
            column={column}
            dataRow={dataRow}
            selectedRows={selectedRows}
            onSelectRow={onSelectRow}
          />
        </div>
      )
    }

    if (isEditing) {
      if (column.header.options) {
        return (
          <GridCellEditSelect
            gridStyle={gridStyle}
            column={column}
            dataRow={dataRow}
            onUpdateCell={this.props.onUpdateCell}
          />
        )
      }

      if (column.header.editor === 'check') {
        return (
          <GridCellEditCheck
            gridStyle={gridStyle}
            column={column}
            dataRow={dataRow}
            onUpdateCell={this.props.onUpdateCell}
          />
        )
      }

      return (
        <GridCellEditText
          gridStyle={gridStyle}
          column={column}
          dataRow={dataRow}
          onUpdateCell={this.props.onUpdateCell}
        />
      )
    }

    return (
      <GridCell
        gridStyle={gridStyle}
        column={column}
        dataRow={dataRow}
        onClickCell={this.props.onClickCell}
        isHighlighted={isHighlightedRow && column === highlightedColumn}
      />
    )
  }
}
