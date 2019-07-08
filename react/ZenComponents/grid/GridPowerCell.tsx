import { React, _ } from './gridImports'
import { IGridStyle } from './IGridStyle'
import { IGridColumn } from './IGridColumn'
import { defaultColumnWidth, defaultColumnHeight } from './gridDefaults'

import DeleteForever from 'material-ui-icons/DeleteForever'
import Delete from 'material-ui-icons/Delete'
import Add from 'material-ui-icons/Add'
import ContentCopy from 'material-ui-icons/ContentCopy'
import Edit from 'material-ui-icons/Edit'
import IconButton from 'material-ui/IconButton'

import Check from 'material-ui-icons/Check'
import CheckCircle from 'material-ui-icons/CheckCircle'

export class GridPowerIcon extends React.Component<
  {
    onClick?: () => void,
  },
  {}
  > {
  render() {
    return (
      <IconButton
        style={{ margin: '-18px -10px 0px -10px' }}
        onClick={this.props.onClick}
      >
        {this.props.children}
      </IconButton>
    )
  }
}

export class GridPowerCellEdit extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn,
  },
  {}
  > {
  render() {
    let width = 100
    let height = defaultColumnHeight

    let { gridStyle, column } = this.props

    return (
      <div style={column.cellStyle}>
        <div style={gridStyle.cellIcons}>
          <GridPowerIcon>
            <CheckCircle />
          </GridPowerIcon>
        </div>
      </div>
    )
  }
}

export class GridPowerCell extends React.Component<
  {
    gridStyle: IGridStyle
    column: IGridColumn
    isEditing: boolean
    isNew: boolean
    onDeleteRow: (dataRow: any) => void
    onEditRow: (dataRow: any) => void
    onCloneRow: (dataRow: any) => void
    onAddRow: (dataRow: any) => void
    dataRow: any,
  },
  {}
  > {
  onDeleteRow = () => {
    this.props.onDeleteRow(this.props.dataRow)
  }
  onEditRow = () => {
    this.props.onEditRow(this.props.dataRow)
  }
  onCloneRow = () => {
    this.props.onCloneRow(this.props.dataRow)
  }
  onAddRow = () => {
    this.props.onAddRow(this.props.dataRow)
  }

  render() {
    let { gridStyle, column } = this.props

    if (this.props.isNew) {
      return (
        <div style={column.cellStyle}>
          <div style={gridStyle.cellIcons}>
            <GridPowerIcon onClick={this.onAddRow}>
              <Add />
            </GridPowerIcon>
          </div>
        </div>
      )
    }

    if (this.props.isEditing) {
      return <GridPowerCellEdit column={column} gridStyle={gridStyle} />
    }

    let width = 100
    let height = defaultColumnHeight
    return (
      <div style={column.cellStyle}>
        <div style={gridStyle.cellIcons}>
          {/* <GridPowerIcon onClick={this.onEditRow}>
            <Edit />
          </GridPowerIcon> */}
          {/* <GridPowerIcon onClick={this.onCloneRow}>
            <ContentCopy />
          </GridPowerIcon> */}
          <GridPowerIcon onClick={this.onDeleteRow}>
            <Delete />
          </GridPowerIcon>
        </div>
      </div>
    )
  }
}
