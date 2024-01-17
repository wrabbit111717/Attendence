import StyledComponent from 'src/components/ButtonGroup/ButtonGroup.styled'

const ButtonGroup = ({ children, ...props }) => {
  return <StyledComponent {...props}>{children}</StyledComponent>
}

export default ButtonGroup
