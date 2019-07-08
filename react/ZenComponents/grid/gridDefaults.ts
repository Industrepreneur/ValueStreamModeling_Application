import { IGridStyle } from './IGridStyle'

const defaultColumnWidth = 150
const defaultColumnHeight = 28

const defaultGridStyle: IGridStyle = {
  grid: {
    padding: '5px',
    margin: '5px',
    fontFamily: 'Roboto',
  },
  gridContainer: {
    overflowX: 'auto',
    overflowY: 'hidden',
  },
  headerRow: {
    display: 'flex',
    borderBottom: 'solid 2px #ccc',
    marginBottom: '0px',
    fontFamily: 'Roboto',
    height: '40px',
    alignItems: 'flex-end',
  },
  header: {
    fontWeight: 'bold',
    color: '#777',
    // height: '40px',
    // minHeight: '40px',
  },
  headerHighlighted: {
    fontWeight: 'bold',
    color: '#444',
  },
  cell: {
    overflow: 'hidden',
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    cursor: 'pointer',
    fontSize: '14px',
    padding: '4px 2px 0px 2px',
    margin: '1px',
  },
  cellCheck: {
    fontSize: '14px',
    padding: '0px 0px 0px 0px',
    margin: '0px',
    overflow: 'hidden',
    minHeight: '40px'
  },
  cellHighlighted: {
    overflow: 'hidden',
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    cursor: 'pointer',
    fontSize: '14px',
    padding: '4px 2px 0px 2px',
    background: '#ddd',
    borderRadius: '4px',
    margin: '1px',
  },
  cellEdit: {
    overflow: 'hidden',
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    cursor: 'pointer',
    fontSize: '14px',
    padding: '0px 4.5px 0px 2px',
    margin: '0px',
  },
  cellIcons: {
    fontSize: '14px',
    paddingTop: '7px',
  },
  row: {
    fontSize: '14px',
    fontFamily: 'Roboto',
    display: 'flex',
    maxHeight: defaultColumnHeight + 'px',
    borderBottom: 'solid 1px #ccc',
  },
  rowHighlighted: {
    fontSize: '14px',
    fontFamily: 'Roboto',
    display: 'flex',
    maxHeight: defaultColumnHeight + 'px',
    borderBottom: 'solid 1px #ccc',
    background: '#eee',
  },
  rowSelected: {
    fontSize: '14px',
    fontFamily: 'Roboto',
    display: 'flex',
    maxHeight: defaultColumnHeight + 'px',
    borderBottom: 'solid 1px #ccc',
    background: '#ddd',
  },
}

export { defaultColumnHeight, defaultColumnWidth, defaultGridStyle }
