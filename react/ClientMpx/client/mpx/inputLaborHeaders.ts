import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

import * as headersConfig from './headersConfig'

export function buildHeaders(showAdvanced) {
  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'laborid',
      width: 40,
      tag: 'id',
    },
    {
      label: 'Group Name',
      dataItem: 'labordesc',
    },
    {
      label: 'Dept',
      dataItem: 'labordept',
    },
    {
      label: 'Qty',
      dataItem: 'grpsiz',
    },
    {
      label: 'Overtime %',
      dataItem: 'ot',
    },
    {
      label: 'Inefficiency',
      dataItem: 'abst',
    },
    {
      label: 'Comment',
      dataItem: 'comment',
    },

    // {
    //   label: 'Dept',
    //   dataItem: 'department',
    // },
    // {
    //   label: 'Qty',
    //   dataItem: 'quantity',
    // },
    // {
    //   label: 'Overtime %',
    //   dataItem: 'overtimepercentage',
    // },
    // {
    //   label: 'Inefficiency',
    //   dataItem: 'inefficiency',
    // },
    // {
    //   label: 'Comment',
    //   dataItem: 'comment',
    // },
  ]

  headers = headersConfig.filterHeaders(headers, showAdvanced)

  return headers
}
