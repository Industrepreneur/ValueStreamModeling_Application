import { React, _ } from './gridImports'
import { IGridStyle } from './IGridStyle'
import { IGridColumn } from './IGridColumn'
import { defaultColumnWidth, defaultColumnHeight } from './gridDefaults'

import { GridRow } from './GridRow'

export class GridRowArea extends React.Component<
  {
    gridStyle: IGridStyle
    columns: IGridColumn[]
    data: any[]
    newItem: any
    editColumn: IGridColumn
    highlightedColumn: IGridColumn
    editDataRow: any
    highlightedDataRow: any
    onClickCell: (ev, column: IGridColumn, dataRow: any) => void
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void
    onDeleteRow: (dataRow: any) => void
    onEditRow: (dataRow: any) => void
    onCloneRow: (dataRow: any) => void
    onAddRow: (dataRow: any) => void,
    selectedRows: any[],
    onSelectRow?: (dataRow: any, isSelected: boolean, mode: 'single' | 'multiple') => void
  },
  any
  > {
  state = {
    editColumn: null,
    editDataRow: null,
  }

  render() {
    let { data, newItem, columns, gridStyle, selectedRows, onSelectRow } = this.props
    let {
      editColumn,
      editDataRow,
      highlightedDataRow,
      highlightedColumn,
    } = this.props
    return (
      <div>
        {_.map(data, (c, cIdx) => (
          <GridRow
            gridStyle={gridStyle}
            key={cIdx}
            columns={columns}
            dataRow={c}
            onClickCell={this.props.onClickCell}
            editColumn={editColumn}
            editDataRow={editDataRow}
            highlightedDataRow={highlightedDataRow}
            highlightedColumn={highlightedColumn}
            onUpdateCell={this.props.onUpdateCell}
            onDeleteRow={this.props.onDeleteRow}
            onEditRow={this.props.onEditRow}
            onCloneRow={this.props.onCloneRow}
            onAddRow={this.props.onCloneRow}
            selectedRows={selectedRows}
            onSelectRow={onSelectRow}
          />
        ))}
        {newItem && (
          <GridRow
            gridStyle={gridStyle}
            key='new-item'
            isNewRow={true}
            columns={columns}
            dataRow={newItem}
            onClickCell={this.props.onClickCell}
            editColumn={editColumn}
            editDataRow={editDataRow}
            highlightedDataRow={highlightedDataRow}
            highlightedColumn={highlightedColumn}
            onUpdateCell={this.props.onUpdateCell}
            onDeleteRow={this.props.onDeleteRow}
            onEditRow={this.props.onEditRow}
            onCloneRow={this.props.onCloneRow}
            onAddRow={this.props.onCloneRow}
            selectedRows={selectedRows}
            onSelectRow={onSelectRow}
          />
        )}

      </div>
    )
  }
}
