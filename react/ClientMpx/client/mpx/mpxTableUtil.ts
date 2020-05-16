import { _ } from '@zen-components/grid/gridExports'

import fetch from 'isomorphic-unfetch'

// MPX helper functions

const debugMode = false

async function mpxFetch(url, body?: any) {
  return await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include', // Send cookies
    body: body ? JSON.stringify(body) : null,
  })
}

// Get table data in a nice format
export async function fetchTableJson(apiName: string) {
  let result = await mpxFetch(`/api/mpx/v1/${apiName}.aspx/selectAll`)

  result = await result.json()
  let data = result.d

  let { Rows, Columns } = data

  Columns = _.map(Columns, (c: string) => c.toLowerCase())

  if (debugMode) {
    console.log('columns', Columns)
  }

  let newData = []
  _.forEach(Rows, row => {
    let newItem: any = {}
    for (let i = 0; i < Columns.length; i++) {
      newItem[Columns[i]] = row[i]
    }
    // console.log('n', newItem)
    newData.push(newItem)
  })

  return newData
}

// Mostly used for examining table data
export async function fetchTableRaw(apiName: string) {
  let result = await mpxFetch(`/api/mpx/v1/${apiName}.aspx/selectAll`)
  result = await result.json()
  let data = result.d
  let { Rows, Columns } = data
  return { Rows, Columns }
}

// Update a single column in a table
export async function updateTable(apiName, id, columnName, newValue) {
  // console.log('update', apiName, id, columnName, newValue)
  let result = await mpxFetch(`/api/mpx/v1/${apiName}.aspx/updateRow`, {
    id,
    columnName,
    newValue,
  })
  return result
}

// Get a list of values
export async function selectList(apiName, columnName) {
  console.log('select list', apiName, columnName)

  let result = await mpxFetch(`/api/mpx/v1/${apiName}.aspx/selectList`, {
    columnName,
  })

  result = await result.json()
  console.log('result', result)

  let data = result.d
  let { Rows, Columns } = data

  return _.flatten(Rows)
}

export async function fetchAddRow(apiName, param1?) {
  return await mpxFetch(`/api/mpx/v1/${apiName}.aspx/addRow`, {
    param1
  })
}

export async function fetchDeleteRow(apiName, id) {
  return await mpxFetch(`/api/mpx/v1/${apiName}.aspx/deleteRow`, {
    id,
  })
}
