import { lazy, Suspense } from 'react'
import { Await, defer, useLoaderData } from 'react-router-dom'
import LoaderSpinner from 'src/components/LoaderSpinner/LoaderSpinner'
import { BriefcaseProvider } from 'src/pages/Create/context/useBriefcase'
import * as api from 'src/services/actions/briefcase'

const CreatePage = lazy(async () => {
  return import('src/pages/Create/CreatePage')
})

const CreateRoute = () => {
  const { briefcase } = useLoaderData()
  return (
    <Suspense fallback={<LoaderSpinner />}>
      <Await resolve={briefcase}>
        {briefcase => (
          <BriefcaseProvider briefcase={briefcase}>
            <CreatePage />
          </BriefcaseProvider>
        )}
      </Await>
    </Suspense>
  )
}

export const briefcaseLoader = ({ params: { briefcaseId } }) => {
  const briefcase = api.fetchBriefcase(briefcaseId?.split('-')[0])
  return defer({
    briefcase
  })
}

export default CreateRoute
