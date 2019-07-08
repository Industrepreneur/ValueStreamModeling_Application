import { React, _ } from './gridImports'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'

import Checkbox from 'material-ui/Checkbox'

export class GridCellCheck extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn
    dataRow: any
    selectedRows: any[]
    onSelectRow?: (dataRow: any, isSelected: boolean, mode: 'single' | 'multiple') => void
  },
  {}
> {
  _onChange = event => {
    let newValue = event.target.checked
    if (this.props.onSelectRow) {
      this.props.onSelectRow(this.props.dataRow, newValue, 'multiple')
    }
  }

  render() {
    let { gridStyle, column, dataRow, selectedRows } = this.props

    let isSelected = selectedRows.indexOf(dataRow) !== -1

    return (
      <div style={gridStyle.cellCheck}>
        <div style={column.cellStyle}>
          <div style={{ margin: '-10px -5px' }}>
            <Checkbox
              checked={isSelected}
              onChange={this._onChange}
              color="primary"
            />
          </div>
        </div>
      </div>
    )
  }
}
