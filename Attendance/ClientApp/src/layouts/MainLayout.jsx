import { Container } from 'react-bootstrap'
import { Outlet } from 'react-router-dom'

const MainLayout = () => {
  return (
    <Container className="py-4">
      <Outlet />
    </Container>
  )
}

export default MainLayout
