import styled from 'styled-components'

const StyledComponent = styled.div`
  & > .btn {
    margin-right: 0.8rem;
  }
  & .btn:last-child {
    margin-right: 0;
  }
`

export default StyledComponent
