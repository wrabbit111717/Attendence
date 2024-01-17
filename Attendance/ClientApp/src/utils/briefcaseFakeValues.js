export default function briefcaseDefaultValues(data) {
  return {
    ...(data ?? undefined),
    vesselId: '4',
    inspectionTypeId: '2',
    inspectionSourceId: '2',
    inspectionCode: null,
    vettingDate: '2023-10-18T15:09',
    inspectorName: '',
    portId: 2989,
    comments: 'test',
    questionnaires: [207]
  }
}
