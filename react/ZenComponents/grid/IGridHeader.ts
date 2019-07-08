import { ILabelValue } from './ILabelValue'

export interface IGridHeader {
  label?: string
  dataItem?: string
  width?: number
  power?: boolean
  check?: boolean
  options?: ILabelValue[]
  tooltip?: string
  editor?: 'check'
  tag?: any
}
