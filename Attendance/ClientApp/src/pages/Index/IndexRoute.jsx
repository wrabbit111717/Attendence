import { lazy, Suspense } from 'react'
import { Await, defer, useLoaderData } from 'react-router-dom'
import LoaderSpinner from 'src/components/LoaderSpinner/LoaderSpinner'
import * as api from 'src/services/actions/briefcase'
import queryClient from 'src/services/queryClient'

const IndexPage = lazy(async () => {
  return import('src/pages/Index/IndexPage')
})

const IndexRoute = () => {
  const { data } = useLoaderData()
  return (
    <Suspense fallback={<LoaderSpinner />}>
      <Await resolve={data}>{resolvedData => <IndexPage briefcases={resolvedData} />}</Await>
    </Suspense>
  )
}

const fetchOptions = ({ page = '1' }) => ({
  queryKey: ['briefcases', 'list', { page }],
  queryFn: ({ signal }) => api.fetchBriefcases(signal),
  initialData: [],
  keepPreviousData: true
})

export const indexLoader = ({ params: { page, filters, order }, request }) => {
  // const url = new URL(request.url)
  // const page = url.searchParams.get('page') ?? '1'
  const data = queryClient.fetchQuery(fetchOptions({ page: page ?? '1' }))
  return defer({ page: page ?? '1', data })
}

export default IndexRoute
