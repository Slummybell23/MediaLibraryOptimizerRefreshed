const STATE_STYLES = {
  Idle: 'border-zinc-700 text-zinc-400',
  Running: 'border-sky-800 bg-sky-950 text-sky-300',
  Completed: 'border-emerald-800 bg-emerald-950 text-emerald-300',
  Failed: 'border-red-800 bg-red-950 text-red-300',
  Cancelled: 'border-amber-800 bg-amber-950 text-amber-300',
}

function formatTime(utcString) {
  return utcString ? new Date(utcString).toLocaleString() : null
}

function ScanCard({ scan, onStart, onCancel }) {
  const state = scan?.state ?? 'Idle'
  const running = state === 'Running'

  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-5">
      <div className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <h2 className="font-medium text-zinc-100">Library scan</h2>
          <span
            className={`rounded-full border px-2.5 py-0.5 text-xs ${STATE_STYLES[state] ?? STATE_STYLES.Idle}`}
          >
            {running && (
              <span className="mr-1.5 inline-block h-1.5 w-1.5 animate-pulse rounded-full bg-sky-400 align-middle" />
            )}
            {state}
          </span>
        </div>

        {running ? (
          <button
            type="button"
            onClick={onCancel}
            className="rounded-lg border border-zinc-700 px-4 py-1.5 text-sm text-zinc-300 hover:bg-zinc-800"
          >
            Cancel
          </button>
        ) : (
          <button
            type="button"
            onClick={onStart}
            className="rounded-lg bg-zinc-100 px-4 py-1.5 text-sm font-medium text-zinc-900 hover:bg-white"
          >
            Start scan
          </button>
        )}
      </div>

      <div className="mt-3 space-y-1 text-sm text-zinc-400">
        {running && (
          <>
            <p>{scan.filesScanned} files scanned</p>
            {scan.currentFile && (
              <p className="truncate font-mono text-xs text-zinc-500">
                {scan.currentFile}
              </p>
            )}
          </>
        )}

        {!running && scan?.completedAtUtc && (
          <p>
            Last scan {state.toLowerCase()} at {formatTime(scan.completedAtUtc)}
            {' — '}
            {scan.filesScanned} files scanned
          </p>
        )}

        {state === 'Failed' && scan?.errorMessage && (
          <p className="text-red-400">{scan.errorMessage}</p>
        )}

        {state === 'Idle' && <p>No scan has run yet.</p>}
      </div>
    </div>
  )
}

export default ScanCard
