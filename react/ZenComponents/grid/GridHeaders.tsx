import { React, _ } from './gridImports'
import { IGridHeader } from './IGridHeader'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'

import { defaultColumnHeight } from './gridDefaults'

export class GridPowerHeader extends React.Component<{}, {}> {
  render() {
    let width = 100
    //let height = defaultColumnHeight
    return (
      <div style={{ width: width + 'px', }}>&nbsp;</div>
    )
  }
}

export class GridHeader extends React.Component<
  {
    column: IGridColumn
    highlightedColumn: IGridColumn
    gridStyle: IGridStyle
  },
  {}
> {
  render() {
    let { gridStyle, column, highlightedColumn } = this.props

    let isHighlighted = column === highlightedColumn

    return (
      <div
        style={column.headerStyle}
        title={column.header.tooltip || column.header.label}
      >
        <div
          style={isHighlighted ? gridStyle.headerHighlighted : gridStyle.header}
        >
          {column.header.label}
        </div>
      </div>
    )
  }
}

export class GridHeaders extends React.Component<
  {
    gridStyle: IGridStyle
    headers: IGridHeader[]
    columns: IGridColumn[]
    highlightedColumn: IGridColumn
  },
  {}
> {
  render() {
    let { gridStyle, headers, columns, highlightedColumn } = this.props

    return (
      <div style={gridStyle.headerRow}>
        {_.map(columns, (c, cIdx) => {
          if (c.header.power) {
            return <GridPowerHeader key="power" />
          } else {
            return (
              <GridHeader
                gridStyle={gridStyle}
                key={cIdx}
                column={c}
                highlightedColumn={highlightedColumn}
              />
            )
          }
        })}
      </div>
    )
  }
}
