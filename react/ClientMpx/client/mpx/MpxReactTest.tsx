import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
} from '@ZenComponents/grid/gridExports'

import fetch from 'isomorphic-unfetch'

const Table = props => (
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

export class MpxReactTest extends React.Component<{}, {}> {
  state = {
    pingResult: '?',
    getResult: null,
    updateResult: null,
  }

  componentWillMount() {
    fetch('/api/mpx/v1/products.aspx/ping', {
      //method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      //body: JSON.stringify({ hungry: true })
    })
      .then(r => r.json())
      .then(data => {
        console.log('r', data)
        this.setState({ pingResult: data })
      })
    fetch('/api/mpx/v1/products.aspx/selectAll', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .then(r => r.json())
      .then(data => {
        console.log('r', data)
        this.setState({ getResult: data })
      })
    fetch('/api/mpx/v1/products.aspx/updateRow', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .then(r => r.json())
      .then(data => {
        console.log('r', data)
        this.setState({ updateResult: data })
      })
  }

  render() {
    let { getResult } = this.state
    return (
      <div>
        MPX React Test OK
        {getResult && (
          <Table columns={getResult.d.Columns} rows={getResult.d.Rows} />
        )}
        <pre>{JSON.stringify(this.state.pingResult, null, 2)}</pre>
        <pre>{JSON.stringify(this.state.updateResult, null, 2)}</pre>
        <pre>{JSON.stringify(this.state.getResult, null, 2)}</pre>
      </div>
    )
  }
}
