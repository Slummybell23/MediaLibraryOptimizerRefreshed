function StatCard({ label, value }) {
  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900 p-5">
      <p className="text-sm text-zinc-400">{label}</p>
      <p className="mt-1 text-3xl font-semibold text-zinc-50">{value}</p>
    </div>
  )
}

export default StatCard
