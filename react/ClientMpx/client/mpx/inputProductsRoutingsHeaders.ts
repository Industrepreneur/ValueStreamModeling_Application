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
      dataItem: 'recid',
      tag: 'id',
      width: 40,
    },
    {
      label: 'Product',
      dataItem: 'proddesc',
      tag: 'parent',
    },
    {
      label: 'Operation From',
      dataItem: 'fromopname',
    },
    {
      label: 'Operation To',
      dataItem: 'toopname',
    },
    {
      label: '% Routed',
      dataItem: 'per',
    },
  ]

  headers = headersConfig.filterHeaders(headers, showAdvanced)

  return headers
}
