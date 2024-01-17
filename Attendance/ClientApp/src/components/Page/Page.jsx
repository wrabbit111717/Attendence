const Page = ({ title, children, ...props }) => {
  document.title = title
  return <div {...props}>{children}</div>
}

export default Page
