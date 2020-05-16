import {
  IGridHeader,
  IGridColumn,
  _,
  gridUtil,
} from '@zen-components/grid/gridExports'

const showIds = false
const showParents = true
export { showIds, showParents }

export function filterHeaders(headers: IGridHeader[], showAdvanced) {
  if (!showAdvanced) {
    headers = _.filter(headers, c => c.tag !== 'advanced')
  }
  if (!showIds) {
    headers = _.filter(headers, c => c.tag !== 'id')
  }
  if (!showParents) {
    headers = _.filter(headers, c => c.tag !== 'parent')
  }
  return headers
}