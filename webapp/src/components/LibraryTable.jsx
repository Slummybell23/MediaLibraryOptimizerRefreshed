function formatBytes(bytes) {
  if (!bytes) return '—'
  const units = ['B', 'KB', 'MB', 'GB', 'TB']
  let value = bytes
  let unit = 0
  while (value >= 1024 && unit < units.length - 1) {
    value /= 1024
    unit++
  }
  return `${value.toFixed(unit === 0 ? 0 : 1)} ${units[unit]}`
}

function LibraryTable({ files }) {
  if (!files.length) {
    return (
      <div className="rounded-xl border border-dashed border-zinc-800 p-10 text-center text-sm text-zinc-500">
        No media files discovered yet. Run a scan to populate the library.
      </div>
    )
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-zinc-800">
      <table className="w-full text-left text-sm">
        <thead className="border-b border-zinc-800 bg-zinc-900 text-xs uppercase tracking-wide text-zinc-500">
          <tr>
            <th className="px-4 py-3 font-medium">File</th>
            <th className="px-4 py-3 font-medium">Size</th>
            <th className="px-4 py-3 font-medium">Codec</th>
            <th className="px-4 py-3 font-medium">DV</th>
            <th className="px-4 py-3 font-medium">Status</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-zinc-800/60">
          {files.map((file) => (
            <tr key={file.id} className="hover:bg-zinc-900/50">
              <td className="max-w-md truncate px-4 py-2.5 font-mono text-xs text-zinc-300">
                {file.filePath}
              </td>
              <td className="px-4 py-2.5 text-zinc-400">
                {formatBytes(file.fileSizeBytes)}
              </td>
              <td className="px-4 py-2.5 text-zinc-400">
                {file.videoCodec ?? '—'}
              </td>
              <td className="px-4 py-2.5 text-zinc-400">
                {file.dolbyVisionProfile != null
                  ? `P${file.dolbyVisionProfile}`
                  : '—'}
              </td>
              <td className="px-4 py-2.5">
                <span className="rounded-full border border-zinc-700 px-2 py-0.5 text-xs text-zinc-400">
                  {file.status}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default LibraryTable
