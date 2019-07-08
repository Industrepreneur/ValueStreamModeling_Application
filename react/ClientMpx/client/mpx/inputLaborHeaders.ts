import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@ZenComponents/grid/gridExports'

export function buildHeaders(showAdvanced) {
  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'laborid',
      width: 40,
    },
    {
      label: 'Group Name',
      dataItem: 'labordesc',
    },
    {
      label: 'Dept',
      dataItem: 'department',
    },
    {
      label: 'Qty',
      dataItem: 'quantity',
    },
    {
      label: 'Overtime %',
      dataItem: 'overtimePercentage',
    },
    {
      label: 'Inefficiency',
      dataItem: 'inefficiency',
    },
    {
      label: 'Comment',
      dataItem: 'comment',
    },
  ]
  return headers
}
