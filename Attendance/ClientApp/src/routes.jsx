import { createBrowserRouter, Navigate } from 'react-router-dom'
import ErrorBoundary from 'src/components/ErrorBoundary/ErrorBoundary'
import MainLayout from 'src/layouts/MainLayout'
import CreateRoute, { briefcaseLoader } from 'src/pages/Create/CreateRoute'
import IndexRoute, { indexLoader } from 'src/pages/Index/IndexRoute'

const routes = createBrowserRouter(
  [
    {
      element: <MainLayout />,
      errorElement: <ErrorBoundary />,
      children: [
        {
          path: '/',
          loader: indexLoader,
          element: <IndexRoute />
        },
        {
          path: 'Item/:briefcaseId?',
          loader: briefcaseLoader,
          element: <CreateRoute />
        }
      ],
      shouldRevalidate: () => false
    },
    {
      path: '*',
      element: <Navigate to="/" replace />
    }
  ],
  {
    basename: '/Briefcases'
  }
)

export default routes
