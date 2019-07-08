import { React, _ } from './gridImports'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'

import Checkbox from 'material-ui/Checkbox'

export class GridCellEditCheck extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn
    dataRow: any
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void
  },
  {}
> {
  _onChange = event => {
    let newValue = event.target.checked
    console.log('change to ', newValue)
    this.props.onUpdateCell(newValue, this.props.column, this.props.dataRow)
  }

  render() {
    let { gridStyle, column, dataRow } = this.props

    let header = column.header
    let item = ''
    if (header.dataItem) {
      item = dataRow[header.dataItem] || ''
    }

    return (
      <div style={gridStyle.cellEdit} title={'' + item}>
        <div style={column.cellStyle}>
          <div style={{margin: '-10px -5px'}}>
            <Checkbox
              checked={item}
              onChange={this._onChange}
              value={column.header.dataItem}
              color="primary"
            />
          </div>
        </div>
      </div>
    )
  }
}
