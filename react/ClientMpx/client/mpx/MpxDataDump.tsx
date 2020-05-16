import * as React from 'react'
import {
  Grid,
  IGridHeader,
  IGridColumn,
  _,
} from '@zen-components/grid/gridExports'

import fetch from 'isomorphic-unfetch'

import { MpxTable } from './MpxTable'
import * as mpxTableUtil from './mpxTableUtil'

// let apiName = 'products-operations'
// let apiName = 'products-routings'
// let apiName = 'products-ibom'
let apiName = 'labor'
export class MpxDataDump extends React.Component<{}, {}> {
  state = {
    data: null as any,
  }

  componentWillMount() {

    this._fetch()
    // fetch('/api/mpx/v1/products.aspx/ping', {
    //   //method: 'POST',
    //   headers: {
    //     'Content-Type': 'application/json',
    //   },
    //   //body: JSON.stringify({ hungry: true })
    // })
    //   .then(r => r.json())
    //   .then(data => {
    //     console.log('r', data)
    //     this.setState({ pingResult: data })
    //   })
    // fetch('/api/mpx/v1/products.aspx/selectAll', {
    //   method: 'POST',
    //   headers: {
    //     'Content-Type': 'application/json',
    //   },
    // })
    //   .then(r => r.json())
    //   .then(data => {
    //     console.log('r', data)
    //     this.setState({ getResult: data })
    //   })
    // fetch('/api/mpx/v1/products.aspx/updateRow', {
    //   method: 'POST',
    //   headers: {
    //     'Content-Type': 'application/json',
    //   },
    // })
    //   .then(r => r.json())
    //   .then(data => {
    //     console.log('r', data)
    //     this.setState({ updateResult: data })
    //   })
  }
  async _fetch() {
    let r = await mpxTableUtil.fetchTableRaw(apiName)
    this.setState({ data: r })
  }

  render() {
    let { data } = this.state
    return (
      <div>
        Fetching MPX data - {apiName}
        {
          data && (
            <MpxTable columns={data.Columns} rows={data.Rows} />
          )
        }
        {/* MPX React Test OK
        {getResult && (
          <Table columns={getResult.d.Columns} rows={getResult.d.Rows} />
        )}
        <pre>{JSON.stringify(this.state.pingResult, null, 2)}</pre>
        <pre>{JSON.stringify(this.state.updateResult, null, 2)}</pre>
        <pre>{JSON.stringify(this.state.getResult, null, 2)}</pre> */}
      </div>
    )
  }
}
