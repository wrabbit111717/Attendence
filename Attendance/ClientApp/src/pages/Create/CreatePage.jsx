import { Button } from 'react-bootstrap'
import { useNavigate } from 'react-router-dom'
import Page from 'src/components/Page/Page'
import useBriefcase from 'src/pages/Create/context/useBriefcase'
import BriefcaseFormLayout from 'src/pages/Create/layouts/BriefcaseFormLayout'

const CreatePage = () => {
  const { briefcase } = useBriefcase()
  const navigate = useNavigate()
  const handleClick = () => {
    navigate('/')
  }
  const title = briefcase?.data ? briefcase.data.inspectionCode : 'Create Briefcase'
  return (
    <Page title={title}>
      <div className="d-flex justify-content-between">
        <h2>{title}</h2>
        <Button onClick={handleClick} className="btn btn-primary">
          &larr; Briefcase List
        </Button>
      </div>
      <BriefcaseFormLayout className="my-3" />
    </Page>
  )
}

export default CreatePage
