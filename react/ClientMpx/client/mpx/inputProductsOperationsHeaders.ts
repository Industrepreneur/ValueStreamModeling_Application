import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

import * as headersConfig from './headersConfig'

export function buildHeaders(showAdvanced, equipmentOptions) {
  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'opid',
      width: 40,
      tag: 'id',
    },
    {
      label: 'Product',
      dataItem: 'proddesc',
      tag: 'parent',
    },
    {
      label: 'Number',
      dataItem: 'opnum',
      tooltip: '',
    },
    {
      label: 'Description',
      dataItem: 'opnam',
      tooltip: '',
    },
    {
      label: 'Equipment',
      dataItem: 'equipdesc',
      options: gridUtil.mapStringsArrayToLabelValueArray(equipmentOptions),
      tooltip: '',
    },
    {
      label: '% Assigned',
      dataItem: 'percentassign',
      tooltip: '',
    },
    {
      label: 'Equipment Setup Time (Lot)',
      dataItem: 'eqsetuptime',
      tooltip: '',
    },
    {
      label: 'Equipment Run Time (Piece)',
      dataItem: 'eqruntime',
      tooltip:
        '',
      width: 90,
    },
    {
      label: 'Labor Setup Time (Lot)',
      dataItem: 'labsetuptime',
      tooltip: '',
      width: 90,
    },
    {
      label: 'Labor Setup Time (Piece)',
      dataItem: 'labruntime',
      tooltip:
        '',
      width: 90,
    },
  ]

  headers = headersConfig.filterHeaders(headers, showAdvanced)

  return headers
}
