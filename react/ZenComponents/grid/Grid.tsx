import { React, _ } from './gridImports'
import { IGridHeader } from './IGridHeader'
import { IGridColumn } from './IGridColumn'
import { IGridStyle } from './IGridStyle'

import { GridHeaders } from './GridHeaders'
import { GridRowArea } from './GridRowArea'
import { GridAddRow } from './GridAddRow'

import {
  defaultColumnWidth,
  defaultColumnHeight,
  defaultGridStyle,
} from './gridDefaults'

import * as gridLayout from './gridLayout'

export class Grid extends React.Component<
  {
    gridStyle?: IGridStyle
    headers: IGridHeader[]    
    data: any[]
    newItem?: any
    onUpdateCell: (newValue: any, column: IGridColumn, dataRow: any) => void
    selectedRows?: any[]
    onSelectRow?: (dataRow: any, isSelected: boolean, mode: 'single' | 'multiple') => void
    onDeleteRow?: (dataRow: any) => void
    onEditRow?: (dataRow: any) => void
    onCloneRow?: (dataRow: any) => void
    onAddRow?: (dataRow: any) => void
  },
  {}
> {
  static defaultProps = {
    gridStyle: defaultGridStyle,
  }

  state = {
    editColumn: null,
    editDataRow: null,
    highlightedDataRow: null,
    highlightedColumn: null,
    columns: [] as IGridColumn[],
  }

  _ref_container: HTMLElement = null
  _set_ref_container = r => {
    this._ref_container = r
  }

  _ref_headers: HTMLElement = null
  _set_ref_headers = r => {
    this._ref_headers = r
  }

  componentWillMount() {
    let columns = gridLayout.layoutGridColumns(this.props.gridStyle, this.props.headers)
    this.setState({ columns })
  }
  componentWillReceiveProps(newProps) {
    let columns = gridLayout.layoutGridColumns(newProps.gridStyle, newProps.headers)
    console.log('update grid')
    this.setState({ columns })
  }

  onClickCell = (ev, column, dataRow) => {    
    this.setState({
      editColumn: column,
      editDataRow: dataRow,
    })
    if(this.props.onSelectRow)
    {
      // console.log(ev)
      // console.log('shift?', ev.shiftKey)
      // console.log('ctrl?', ev.ctrlKey )

      let mode: any = (ev.shiftKey || ev.ctrlKey) ? 'multiple' : 'single'

      this.props.onSelectRow(dataRow, true, mode)
    }
  }

  updateMouse = event => {
    let newHighlightedDataRow = null
    let newHighlightedColumn = null

    let { columns } = this.state

    if (event && this._ref_container && this._ref_headers) {
      let rect = this._ref_container.getBoundingClientRect()
      // let x = event.pageX - this._ref_container.offsetLeft
      // let y = event.pageY - this._ref_container.offsetTop
      let x = event.pageX - rect.left - document.documentElement.scrollLeft
      let y = event.pageY - rect.top - document.documentElement.scrollTop

      // Find this highlighted data row
      let columnHeight = defaultColumnHeight + 1
      // console.log(this._ref_headers.clientHeight, this._ref_headers.offsetHeight)
      let selectedRow = Math.floor(
        (y - this._ref_headers.clientHeight - 2) / columnHeight
      )
      let { data } = this.props
      if (data && data.length > selectedRow && selectedRow >= 0) {
        newHighlightedDataRow = data[selectedRow]
      }

      // Find the highlighted column
      let xAccum = 0
      _.forEach(columns, c => {
        // console.log(x, xAccum, xAccum + c.width)
        if (x >= xAccum && x <= xAccum + c.width) {
          newHighlightedColumn = c
          // console.log('hit', x, xAccum, c.width)
        }
        xAccum += c.width
      })
    }

    if (
      this.state.highlightedDataRow !== newHighlightedDataRow ||
      this.state.highlightedColumn !== newHighlightedColumn
    ) {
      this.setState({
        highlightedDataRow: newHighlightedDataRow,
        highlightedColumn: newHighlightedColumn,
      })
    }
  }

  render() {
    let { headers, data, newItem, gridStyle, selectedRows, onSelectRow } = this.props

    let {
      columns,
      editColumn,
      editDataRow,
      highlightedColumn,
      highlightedDataRow,
    } = this.state

    // calculate width
    let w = 0
    _.forEach(columns, c => {
      w += c.width
    })

    return (
      <div style={gridStyle.gridContainer}>
        <div
          style={{
            width: w + 'px',
            minWidth: '100px',
          }}
          ref={this._set_ref_container}
          onMouseEnter={this.updateMouse}
          onMouseLeave={this.updateMouse}
          onMouseMove={this.updateMouse}
        >
          <div ref={this._set_ref_headers}>
            <GridHeaders
              gridStyle={gridStyle}
              headers={headers}
              columns={columns}
              highlightedColumn={highlightedColumn}
            />
          </div>
          <GridRowArea
            gridStyle={gridStyle}
            columns={columns}
            data={data}
            newItem={newItem}
            onUpdateCell={this.props.onUpdateCell}
            onDeleteRow={this.props.onDeleteRow}
            onEditRow={this.props.onEditRow}
            onCloneRow={this.props.onCloneRow}
            onAddRow={this.props.onAddRow}
            onClickCell={this.onClickCell}
            editColumn={editColumn}
            editDataRow={editDataRow}
            highlightedColumn={highlightedColumn}
            highlightedDataRow={highlightedDataRow}
            selectedRows={selectedRows}
            onSelectRow={onSelectRow}
          />
        </div>
      </div>
    )
  }
}
