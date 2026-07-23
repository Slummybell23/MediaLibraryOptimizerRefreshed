import { useEffect, useState } from 'react'

function StatCard({ label, value }) {
  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-5">
      <p className="text-sm text-zinc-400">{label}</p>
      <p className="mt-1 text-3xl font-semibold text-zinc-50">{value}</p>
    </div>
  )
}

function App() {
  const [status, setStatus] = useState(null)
  const [error, setError] = useState(null)

  useEffect(() => {
    fetch('/api/system/status')
      .then((res) => {
        if (!res.ok) throw new Error(`API returned ${res.status}`)
        return res.json()
      })
      .then(setStatus)
      .catch((err) => setError(err.message))
  }, [])

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
            Could not reach the backend: {error}
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

        <section className="mt-10">
          <h2 className="text-sm font-medium uppercase tracking-wide text-zinc-500">
            Library
          </h2>
          <div className="mt-3 rounded-xl border border-dashed border-zinc-800 p-10 text-center text-sm text-zinc-500">
            Library scanning is not implemented yet. Discovered media files will
            appear here.
          </div>
        </section>
      </main>
    </div>
  )
}

export default App
