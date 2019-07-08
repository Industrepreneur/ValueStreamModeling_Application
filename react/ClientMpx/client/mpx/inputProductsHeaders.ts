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
      dataItem: 'prodid',
      width: 40,
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
  ]

  if (!showAdvanced) {
    headers = _.filter(headers, c => c.tag !== 'advanced')
  }

  return headers
}
