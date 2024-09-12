f = "live.il"
open(f"{f}.stripped", "w").write(
    "\n".join(
        [
            l.strip()
            for l in open(f, "r").read().split("\n")
            if ": nop" not in l and not l.strip().startswith("//")
        ]
    )
)
# print(open("live.il", "r").read())
