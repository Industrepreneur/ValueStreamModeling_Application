import { React, _ } from './gridImports'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'

export class GridCellEditSelect extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn
    dataRow: any
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void
  },
  {}
> {
  _onChange = event => {
    let newValue = event.target.value

    if (newValue === 'null') {
      newValue = null
    }

    console.log(
      `Select: change ${this.props.column.header.dataItem} to ${newValue}`
    )
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
          <select
            onChange={this._onChange}
            value={'' + item}
            style={{ marginTop: '4px', fontFamily: 'Roboto' }}
          >
            {_.map(column.header.options, (c, cIdx) => (
              <option key={'' + c.value} value={'' + c.value}>
                {c.label}
              </option>
            ))}
          </select>
        </div>
      </div>
    )
  }
}
