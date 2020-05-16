import { _ } from '@zen-components/grid/gridExports'

// These are for an out-of-date demo
export function createFakeProductData() {
  let data = []
  for (let i = 0; i < 10; i++) {
    data.push({
      ProdID: 100 + i,
      ProdDesc: 'item ' + (i + 1),
      ProdDept: 'PF ' + i,
      EndDemd: _.random(1, 5, false),
      Lotsiz: _.random(1, 2, false),
      ProdComment: 'A comment for item ' + i,
    })
  }
  return data
}

export function createFakeEquipmentData() {
  let data = []
  for (let i = 0; i < 10; i++) {
    data.push({
      id: 100 + i,
      name: 'item ' + (i + 1),
      department: 'dept ' + i,
      type: _.sample(['Standard', 'Delay']),
      quantity: _.random(1, 20, false),
      overtimePercentage: _.random(1, 20, false),
      MTTF: 0,
      MTTR: 0,
      label: 'none',
      comment: 'A comment for item ' + i,
    })
  }
  return data
}

export function createFakeLaborData() {
  let data: any = []
  for (let i = 0; i < 10; i++) {
    data.push({
      id: 100 + i,
      groupName: 'item ' + (i + 1),
      department: 'dept ' + i,
      quantity: _.random(1, 20, false),
      overtimePercentage: _.random(1, 20, false),
      inefficiency: 0,
      comment: 'A comment for item ' + i,
    })
  }
  this.setState({ data })
}
