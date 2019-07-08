import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@ZenComponents/grid/gridExports'

export function  buildHeaders(showAdvanced, laborOptions) {
  let headers: IGridHeader[] = [
    {
      check: true,
    },
    {
      label: 'ID',
      dataItem: 'equipid',
      width: 40,
    },
    {
      label: 'Name',
      dataItem: 'equipdesc',
      tooltip: 'Name of the equipment group (each row must be unique).',
    },
    {
      label: 'Dept',
      dataItem: 'equipdept',
      tooltip: 'Department of the equipment group (for user only).',
    },
    {
      label: 'Type',
      dataItem: 'equiptypename',
      options: [
        { label: '-', value: 'null' },
        { label: 'Standard', value: 'Standard' },
        { label: 'Delay', value: 'Delay' },
      ],
      // dataItem: 'equiptype',
      // options: [
      //   { label: '-', value: 'null' },
      //   { label: 'Standard', value: '0' },
      //   { label: 'Delay', value: '1' },
      // ],
      tooltip:
        'Standard has a finite number of machines and Delay has infinite number of machines and work is immediately processed',
    },
    {
      label: 'Qty',
      dataItem: 'grpsiz',
      tooltip: 'The number of equipment in the group (at least 1).',
      width: 90,
    },
    {
      label: 'Overtime %',
      dataItem: 'ot',
      tooltip:
        'What extra percent of the scheduled time this equipment group can be worked.',
      width: 90,
    },
    {
      label: 'MTTF',
      dataItem: 'mtf',
      tooltip:
        'The average amount of time that the equipment can run before breaking, measured in the Operations Time Units selected on the General Input Page.',
      width: 90,
    },
    {
      label: 'MTTR',
      dataItem: 'mtr',
      tooltip:
        'The average amount of time that the equipment takes to be repaired, measured in the Operations Time Units selected on the General Input Page.',
      width: 90,
    },
    {
      label: 'Labor',
      dataItem: 'labordesc',
      options: gridUtil.mapStringsArrayToLabelValueArray(laborOptions),
      tooltip: 'The labor group that operates the equipment.',
    },
    {
      label: 'Setup Time Multiplier',
      dataItem: 'setup',
      tooltip:
        'A multiplier for how long of the input set-up time the machine group takes.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Run Time Multiplier',
      dataItem: 'run',
      tooltip:
        'A multiplier for how long of the input run time the machine group takes. This is useful to quickly effect ALL run times that the machine group does.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Variability Multiplier',
      dataItem: 'varbility',
      tooltip:
        'A multiplier for how much of the Variability in Product Times affect this machine group.',
      width: 90,
      tag: 'advanced',
    },
    {
      label: 'Comment',
      dataItem: 'EqComment',
    },
  ]

  if (!showAdvanced) {
    headers = _.filter(headers, c => c.tag !== 'advanced')
  }

  return headers
}