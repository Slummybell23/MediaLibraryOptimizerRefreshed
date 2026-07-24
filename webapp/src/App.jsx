import { useCallback, useEffect, useState } from 'react'
import { getJson, postJson } from './api'
import StatCard from './components/StatCard'
import ScanCard from './components/ScanCard'
import LibraryTable from './components/LibraryTable'

function App() {
  const [status, setStatus] = useState(null)
  const [scan, setScan] = useState(null)
  const [files, setFiles] = useState([])
  const [error, setError] = useState(null)

  const refresh = useCallback(async () => {
    try {
      const [systemStatus, scanStatus, mediaFiles] = await Promise.all([
        getJson('/api/system/status'),
        getJson('/api/scan/status'),
        getJson('/api/media-files'),
      ])
      setStatus(systemStatus)
      setScan(scanStatus)
      setFiles(mediaFiles)
      setError(null)
    } catch (err) {
      setError(err.message)
    }
  }, [])

  useEffect(() => {
    refresh()
  }, [refresh])

  // Poll the scan status while a scan is running; refresh everything once it stops.
  useEffect(() => {
    if (scan?.state !== 'Running') return
    const id = setInterval(async () => {
      try {
        const scanStatus = await getJson('/api/scan/status')
        setScan(scanStatus)
        if (scanStatus.state !== 'Running') refresh()
      } catch {
        // Keep polling; transient errors surface via the next refresh.
      }
    }, 1500)
    return () => clearInterval(id)
  }, [scan?.state, refresh])

  const startScan = async () => {
    try {
      setScan(await postJson('/api/scan'))
      setError(null)
    } catch (err) {
      setError(err.message)
    }
  }

  const cancelScan = async () => {
    try {
      setScan(await postJson('/api/scan/cancel'))
    } catch (err) {
      setError(err.message)
    }
  }

  return (
    <div className="min-h-screen bg-zinc-950 text-zinc-100">
      <header className="border-b border-zinc-800">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <h1 className="text-lg font-semibold tracking-tight">
            Media Library Optimizer
          </h1>
          <span className="rounded-full border border-zinc-700 px-3 py-1 text-xs text-zinc-400">
            {status ? 'Connected' : error ? 'API unreachable' : 'Connecting…'}
          </span>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-6 py-8">
        {error && (
          <div className="mb-6 rounded-lg border border-red-900 bg-red-950 p-4 text-sm text-red-300">
            {error}
          </div>
        )}

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <StatCard label="Media files" value={status?.mediaFileCount ?? '—'} />
          <StatCard label="Pending jobs" value={status?.pendingJobCount ?? '—'} />
          <StatCard label="Running jobs" value={status?.runningJobCount ?? '—'} />
        </div>

        {status && !status.mediaPathExists && (
          <div className="mt-6 rounded-lg border border-amber-900 bg-amber-950 p-4 text-sm text-amber-300">
            Media path <code className="font-mono">{status.mediaPath}</code> does
            not exist. Mount your media library to this location.
          </div>
        )}

        <section className="mt-6">
          <ScanCard scan={scan} onStart={startScan} onCancel={cancelScan} />
        </section>

        <section className="mt-10">
          <h2 className="text-sm font-medium uppercase tracking-wide text-zinc-500">
            Library
          </h2>
          <div className="mt-3">
            <LibraryTable files={files} />
          </div>
        </section>
      </main>
    </div>
  )
}

export default App
