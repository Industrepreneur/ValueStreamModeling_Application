import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

import * as headersConfig from './headersConfig'

export function buildHeaders(showAdvanced, selectedProductName, allProductNames) {

  let productNames = [{ label: '-', value: 'null' }]
  _.forEach(allProductNames, (c) => {
    productNames.push({ label: c, value: c })
  })
  // Don't allow to select itself (avoid infinite loop)
  _.remove(productNames, (c) => c.value === selectedProductName)

  console.log(allProductNames)

  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'ibomid',
      width: 40,
      tag: 'id',
    },
    {
      label: 'Product',
      dataItem: 'parentname',
      tag: 'parent',
    },
    {
      label: 'Name',
      dataItem: 'compname',
      options: productNames,
    },
    {
      label: 'Units for Assembly',
      dataItem: 'upa',
      tooltip: '',
    },
  ]

  headers = headersConfig.filterHeaders(headers, showAdvanced)

  return headers
}
