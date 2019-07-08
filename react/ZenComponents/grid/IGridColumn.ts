import { React } from './gridImports'
import { IGridHeader } from './IGridHeader'

export interface IGridColumn {
  header: IGridHeader
  headerStyle: React.CSSProperties
  width: number
  height: number
  cellStyle: React.CSSProperties
  inputStyle: React.CSSProperties
}
