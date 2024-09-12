from pathlib import Path

infile = Path("code.il")
lines = open(infile, "r").read().strip().split("\n")
# print(lines)
print(len(lines), "lines")
need_branches_to = (
    {}
)  # format: keys are instructions that need to be branched to and values are the addresses of the instructions that want to branch to the key address
tmp = []
for line in lines:
    line = line.strip()
    if line.startswith("//"):
        tmp.append([line])
        continue
    _split = line.split(": ")
    if len(_split) > 1:
        address, full_instruction = _split
        instruction_parts = full_instruction.split(" ")
        instruction_code = instruction_parts[0]
        instruction_arguments = instruction_parts[1:]
        if len(instruction_arguments) > 0:  # instruction has arguments
            if len(instruction_arguments) > 1:
                instruction_arguments = [" ".join(instruction_arguments)]
            if instruction_arguments[0].startswith(
                "IL"
            ):  # this is a branch instruction!
                if instruction_arguments[0] not in need_branches_to:
                    need_branches_to[instruction_arguments[0]] = []
                need_branches_to[instruction_arguments[0]].append(address)
        # print(address, instruction_code, instruction_arguments)
        tmp.append([address, instruction_code, instruction_arguments])
    else:  # empty lines
        tmp.append(_split)

to_insert = []
address_to_label_number = {}
label_n = 0
for i in range(len(tmp)):
    v = tmp[i]
    if v[0] == "":
        continue
    if v[0].startswith("//"):
        continue
    if v[0] in need_branches_to:
        to_insert.append([i, label_n])
        address_to_label_number[v[0]] = label_n
        # print(i, v)
        label_n += 1
to_insert = sorted(to_insert, key=lambda e: e[0])
items_inserted = 0
for i, ln in to_insert:
    i += items_inserted
    tmp.insert(i, [f"il.MarkLabel(l[{ln}]);"])
    items_inserted += 1
print(address_to_label_number)
for v in tmp:
    if v[0] == "" or v[0].startswith("//") or len(v) < 2:
        continue
    # print(v)
    if len(v[2]) < 1:
        continue
    # print(v[2])
    if v[2][0].startswith("IL"):
        v[2] = [f"l[{address_to_label_number[v[2][0]]}]"]
for t in tmp:
    print(t)

new_tmp = []
for v in tmp:
    if v[0] == "" or v[0].startswith("//") or len(v) < 2:
        new_tmp.append(v[0])
        continue
    v = v[1:]
    v[0] = "OpCodes." + "_".join([x[0].upper() + x[1:] for x in v[0].split(".")])
    v[1] = "".join(v[1])
    if v[1] == "":
        v.pop()
    # print(v)
    v = f"il.Emit({", ".join(v)});"
    # print(v)
    new_tmp.append(v)
for t in new_tmp:
    print(t)
Path(infile.stem + ".cs").write_text("\n".join(new_tmp))
# print(infile.stem)
# print(need_branches_to)
