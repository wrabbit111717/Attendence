import { Alert, Col, Row } from 'react-bootstrap'
import { useRouteError } from 'react-router-dom'

const ErrorBoundary = ({ onClose, message }) => {
  const error = useRouteError()
  console.log(error)
  return (
    <Row>
      <Col>
        <Alert variant="danger" onClose={onClose} dismissible={!!onClose}>
          <Alert.Heading>Error!</Alert.Heading>
          <p>{message || error?.request?.statusText || 'Unknown error!'}</p>
        </Alert>
      </Col>
    </Row>
  )
}
export default ErrorBoundary
