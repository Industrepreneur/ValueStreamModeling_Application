
import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
} from '@zen-components/grid/gridExports'

const MpxTable = (props: { columns: any[], rows: any[] }) => (
  <div>
    <table>
      <thead>
        <tr>{_.map(props.columns, (c, cIdx) => <th key={cIdx}>{c}</th>)}</tr>
      </thead>
      <tbody>
        {_.map(props.rows, (c: any, cIdx) => (
          <tr key={cIdx}>{_.map(c, (d, dIdx) => <td key={dIdx}>{d}</td>)}</tr>
        ))}
      </tbody>
    </table>
  </div>
)
export { MpxTable }