import { Alert, Col, Row } from 'react-bootstrap'

const SuccessBoundary = ({ onClose, message }) => {
  return (
    <Row>
      <Col>
        <Alert variant="success" onClose={onClose} dismissible={!!onClose}>
          <Alert.Heading>Success!</Alert.Heading>
          <p>{message || '-'}</p>
        </Alert>
      </Col>
    </Row>
  )
}
export default SuccessBoundary
