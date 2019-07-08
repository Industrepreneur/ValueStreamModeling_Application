import { React, _ } from './gridImports'

export function mapStringsArrayToLabelValueArray(options: string[]) {
  return _.map(options as any[], c => {
    return {
      label: c,
      value: c,
    }
  })
}

export function selectRow(selectedRows, dataRow, isSelected, mode) {
  if (mode === 'multiple') {
    let clone = _.clone(selectedRows)
    if (isSelected) {
      if (clone.indexOf(dataRow) === -1) {
        clone.push(dataRow)
      }
    } else {
      _.pull(clone, dataRow)
    }
    return clone
  }
  if (mode === 'single') {
    if (isSelected) {
      return [dataRow]
    }
  }

  return []
}
