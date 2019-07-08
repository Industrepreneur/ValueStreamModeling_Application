import { _ } from '@ZenComponents/grid/gridExports'

import fetch from 'isomorphic-unfetch'

// MPX helper functions

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
export async function fetchTableJson(tableName: string) {
  let result = await mpxFetch(`/api/mpx/v1/${tableName}.aspx/selectAll`)

  result = await result.json()
  let data = result.d

  let { Rows, Columns } = data

  Columns = _.map(Columns, (c: string) => c.toLowerCase())

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

// Update a single column in a table
export async function updateTable(tableName, id, columnName, newValue) {
  console.log('update', tableName, id, columnName, newValue)

  let result = await mpxFetch(`/api/mpx/v1/${tableName}.aspx/updateRow`, {
    id,
    columnName,
    newValue,
  })

  result = await result.json()
  console.log('result', result)
}

// Get a list of values
export async function selectList(tableName, columnName) {
  console.log('select list', tableName, columnName)

  let result = await mpxFetch(`/api/mpx/v1/${tableName}.aspx/selectList`, {
    columnName,
  })

  result = await result.json()
  console.log('result', result)

  let data = result.d
  let { Rows, Columns } = data

  return _.flatten(Rows)
}

export async function fetchAddRow(tableName) {
  let result = await mpxFetch(`/api/mpx/v1/${tableName}.aspx/addRow`)
}

export async function fetchDeleteRow(tableName, id) {
  let result = await mpxFetch(`/api/mpx/v1/${tableName}.aspx/deleteRow`, {
    id,
  })
}
