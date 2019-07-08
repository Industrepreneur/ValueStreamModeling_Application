import { React, _ } from './gridImports'
import { IGridStyle } from './IGridStyle'
import { IGridColumn } from './IGridColumn'
import { defaultColumnWidth, defaultColumnHeight } from './gridDefaults'

import { GridRowCellPicker } from './GridRowCellPicker'
export class GridRow extends React.Component<
  {
    gridStyle: IGridStyle
    isNewRow?: boolean
    columns: IGridColumn[]
    dataRow: any
    editColumn: IGridColumn
    editDataRow: any
    highlightedDataRow: any
    highlightedColumn: IGridColumn
    onClickCell: (ev, column: IGridColumn, dataRow: any) => void
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void

    onDeleteRow: (dataRow: any) => void
    onEditRow: (dataRow: any) => void
    onCloneRow: (dataRow: any) => void
    onAddRow: (dataRow: any) => void
    selectedRows: any[],
    onSelectRow?: (dataRow: any, isSelected: boolean, mode: 'single' | 'multiple') => void
  },
  {}
> {
  render() {
    let {
      gridStyle,
      columns,
      dataRow,
      editDataRow,
      highlightedDataRow,
      highlightedColumn,
      isNewRow,
      selectedRows,
      onSelectRow,
    } = this.props

    let isHighlightedRow = dataRow === highlightedDataRow

    let style = gridStyle.row
    let isEditing = editDataRow === dataRow
    if (isEditing) {
      style = gridStyle.rowSelected
    } else if (isHighlightedRow) {
      style = gridStyle.rowHighlighted
    }

    return (
      <div style={style}>
        {_.map(columns, (c, cIdx) => (
          <GridRowCellPicker
            key={cIdx}
            column={c}
            isEditing={isEditing}
            gridStyle={gridStyle}
            dataRow={dataRow}
            onUpdateCell={this.props.onUpdateCell}
            onClickCell={this.props.onClickCell}
            isHighlightedRow={isHighlightedRow}
            highlightedColumn={highlightedColumn}
            selectedRows={selectedRows}
            onSelectRow={onSelectRow}
          />
        ))}
      </div>
    )
  }
}
