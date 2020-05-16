import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

import { Link } from 'react-router-dom'

import * as headersConfig from './headersConfig'

export function buildHeaders(showAdvanced) {

  const linkStyle = { color: '#3E93D8', textDecoration: 'none' }

  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'prodid',
      width: 40,
      tag: 'id',
    },
    {
      label: 'Name',
      dataItem: 'proddesc',
      tooltip: 'Name of the product, must be unique',
    },
    {
      label: 'Product Family',
      dataItem: 'proddept',
      tooltip: 'Name to group products by',
    },
    {
      label: 'Demand',
      dataItem: 'enddemd',
      tooltip:
        'The total customer demand for the product over the forecast period',
      width: 90,
    },
    {
      label: 'Lot Size',
      dataItem: 'lotsiz',
      tooltip: 'Average lot size used during production of the product',
      width: 90,
    },
    {
      label: 'Batch Size',
      dataItem: 'transferbatch',
      tooltip:
        'Number of pieces completed and pushed on to next operation before whole lot is completed. A value of -1 means that pieces wait until the entire batch is finished before moving.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Gather Batches?',
      dataItem: 'tbatchgather',
      editor: 'check',
      tooltip:
        'Gather all of the pieces transfer batches back into a full lot before moving them to a new product or move the transfer batches to the other product as batches are finished.',
      tag: 'advanced',
    },
    {
      label: 'Make to Stock',
      dataItem: 'makestock',
      editor: 'check',
      tooltip: 'Sets lead time for these parts to 0 in IBOM output page.',
      tag: 'advanced',
    },
    {
      label: 'Priority',
      dataItem: 'value',
      tooltip:
        'Weighting the relative importance of different products with larger numbers meaning the product is more valuable.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Variability Multiplier',
      dataItem: 'variability',
      tooltip:
        'A multiplier for how much of the Variability in Product Times affect this machine group.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Lot Size Multiplier',
      dataItem: 'lotsizefac',
      tooltip: 'A multiplier to affect the Batch Size for the product.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Demand Multiplier',
      dataItem: 'demandfac',
      tooltip: 'A multiplier to affect the Demand for the product.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Comment',
      dataItem: 'prodcomment',
    },

    {
      label: 'Links',
      dataItem: 'proddesc',
      canEdit: false,
      renderer: (dataRow, column, item) => {
        item = encodeURIComponent(item)
        return (<span>
          <Link style={linkStyle} to={`/input/products.aspx?mode=operations&product=${item}`}>Operations</Link> |&nbsp;
          <Link style={linkStyle} to={`/input/products.aspx?mode=routings&product=${item}`}>Routings</Link> |&nbsp;
          <Link style={linkStyle} to={`/input/products.aspx?mode=ibom&product=${item}`}>IBOM</Link>
          {/* <a style={linkStyle} href={`/input/products-operations.aspx?product=${item}`}>Operations</a> |&nbsp;
          <a style={linkStyle} href={`/input/products-routings.aspx?product=${item}`}>Routings</a> |&nbsp;
          <a style={linkStyle} href={`/input/products-ibom.aspx?product=${item}`}>IBOM</a> */}
        </span>)
      },
      width: 200,

    },

  ]

  headers = headersConfig.filterHeaders(headers, showAdvanced)

  return headers
}
