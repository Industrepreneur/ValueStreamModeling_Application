import { React, _ } from './gridImports'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'
import { FaIcon } from '../common/FaIcon'

export class GridCell extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn
    dataRow: any
    onClickCell: (ev, column: IGridColumn, dataRow: any) => void
    isHighlighted: boolean
  },
  {}
> {
  onClick = (ev) => {
    this.props.onClickCell(ev, this.props.column, this.props.dataRow)
  }

  render() {
    let { gridStyle, column, dataRow, isHighlighted } = this.props

    let header = column.header
    let item: any = ''
    if (header.dataItem) {
      item = dataRow[header.dataItem] || ''
    }
    if (header.editor === 'check') {
      let isChecked = item === true || item === 'true'

      return (
        <div
          onMouseDown={this.onClick}
          style={isHighlighted ? gridStyle.cellHighlighted : gridStyle.cell}
          title={isChecked ? 'Yes' : 'No'}
        >
          <div style={column.cellStyle}>
            <FaIcon prefix="far" icon={isChecked ? 'check-square' : 'square'} />{' '}
          </div>
        </div>
      )
    }
    if (header.options) {
      item = '' + item
      let found = _.find(header.options, c => c.value === item)
      if (found) {
        item = found.label
      } else {
        item = '-'
      }
    }

    return (
      <div
        onMouseDown={this.onClick}
        style={isHighlighted ? gridStyle.cellHighlighted : gridStyle.cell}
        title={'' + item}
      >
        <div style={column.cellStyle}>{'' + item}</div>
      </div>
    )
  }
}
