import { React, _ } from './gridImports'
import { IGridHeader } from './IGridHeader'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'


import {
  defaultColumnWidth,
  defaultColumnHeight,
  defaultGridStyle,
} from './gridDefaults'

export function layoutGridColumns(gridStyle: IGridStyle, headers: IGridHeader[]) {
  let columns: IGridColumn[] = []

  let maxWidth = 0
  
  _.forEach(headers, c => {
    let width = c.width || defaultColumnWidth

    if (c.power) {
      width = c.width || 100
    }
    if (c.check) {
      width = c.width || 40
    }

    let headerStyle = {
      width: width + 'px',
      maxWidth: width + 'px',
      // height: defaultColumnHeight + 'px',
      // minHeight: defaultColumnHeight + 'px',
    }

    //let cellStyle = _.clone(gridStyle.cell)
    let cellStyle = {
      width: width + 'px',
      minWidth: width + 'px',
      maxWidth: width + 'px',
      height: defaultColumnHeight + 'px',
      minHeight: defaultColumnHeight + 'px',
    }

    let inputStyle = {
      width: width - 8 + 'px',
      height: defaultColumnHeight + 'px',
      border: 'none',
      outline: 'none',
      fontSize: '14px',
      padding: '0px',
      fontFamily: 'Roboto',
      background: 'transparent',
    }

    let column = {
      header: c,
      width: width,
      height: defaultColumnHeight,
      headerStyle,
      cellStyle,
      inputStyle,
    }
    columns.push(column)

    maxWidth += width
  })

  return columns
}