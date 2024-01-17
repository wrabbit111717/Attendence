import { format } from 'date-fns'
import {
  ButtonGroup as BsButtonGroup,
  Button,
  Card,
  CardGroup,
  Dropdown,
  Jumbotron,
  SplitButton
} from 'react-bootstrap'
import { useNavigate } from 'react-router-dom'
import Page from 'src/components/Page/Page'

const IndexPage = ({ briefcases }) => {
  const navigate = useNavigate()
  const handleClick = () => {
    navigate('/Item')
  }
  console.log(briefcases)
  const user = briefcases.at(0)?.user.fullName ?? ''
  const title = `Briefcases - ${user}`

  return (
    <Page title={title}>
      <Jumbotron className="py-3 bg-light">
        <h2>Briefcases</h2>
        <p>{user}</p>
        <p>
          <Button onClick={handleClick} className="btn btn-primary">
            Add Briefcase
          </Button>
        </p>
      </Jumbotron>
      <CardGroup>
        {briefcases.map(({ vessel: { name: vesselName }, vettingDate, ...briefcase }) => {
          return (
            <Card key={briefcase.id}>
              <Card.Body>
                <Card.Title>
                  {vesselName}
                  <small className="text-mute">{briefcase.inspectionType.name}</small>
                </Card.Title>
                <Card.Text>
                  {briefcase.portName} - {briefcase.portCountry}
                </Card.Text>
                <p>
                  <small>{briefcase.comments}</small>
                </p>
                <SplitButton
                  as={BsButtonGroup}
                  id={`${briefcase.id}-dropdown-button`}
                  title="Report">
                  <Dropdown.Item eventKey="1">Report</Dropdown.Item>
                  <Dropdown.Divider />
                  <Dropdown.Item eventKey="2">Edit</Dropdown.Item>
                  <Dropdown.Item eventKey="3">
                    <span className="text-danger">Delete</span>
                  </Dropdown.Item>
                  <Dropdown.Divider />
                  <Dropdown.Item eventKey="4">Transfer</Dropdown.Item>
                </SplitButton>
              </Card.Body>
              <Card.Footer>
                <small className="text-muted">{format(new Date(vettingDate), 'PPPP p')}</small>
              </Card.Footer>
            </Card>
          )
        })}
      </CardGroup>
    </Page>
  )
}

export default IndexPage
