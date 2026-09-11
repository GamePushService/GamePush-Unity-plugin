"""Generate GamePush overlay chrome sprites (rounded panel + white glyphs)."""
import math
import os
import struct
import zlib

ROOT = os.path.join(
    os.path.dirname(__file__),
    "..",
    "Demo",
    "Assets",
    "Plugins",
    "GamePush",
    "Resources",
    "GamePush",
    "Overlays",
    "Generated",
)


def png(width, height, pixels):
    raw = b"".join(b"\x00" + bytes(pixels[y * width * 4 : (y + 1) * width * 4]) for y in range(height))

    def chunk(tag, data):
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    return (
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
        + chunk(b"IDAT", zlib.compress(raw, 9))
        + chunk(b"IEND", b"")
    )


def write_png(path, width, height, pixels):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as handle:
        handle.write(png(width, height, pixels))


def clamp01(value):
    return 0.0 if value < 0 else 1.0 if value > 1 else value


def aa(distance):
    return clamp01(0.5 - distance)


def set_a(pixels, size, x, y, alpha):
    if alpha <= 0 or x < 0 or y < 0 or x >= size or y >= size:
        return
    i = (y * size + x) * 4
    current = pixels[i + 3] / 255.0
    if alpha > current:
        pixels[i] = 255
        pixels[i + 1] = 255
        pixels[i + 2] = 255
        pixels[i + 3] = int(alpha * 255)


def stamp(pixels, size, cx, cy, radius_fn):
    for y in range(size):
        for x in range(size):
            set_a(pixels, size, x, y, aa(radius_fn(x + 0.5, y + 0.5)))


def dist_segment(px, py, ax, ay, bx, by):
    abx, aby = bx - ax, by - ay
    length = abx * abx + aby * aby
    t = 0.0 if length == 0 else clamp01(((px - ax) * abx + (py - ay) * aby) / length)
    dx, dy = px - (ax + abx * t), py - (ay + aby * t)
    return math.hypot(dx, dy)


def stroke_segment(pixels, size, ax, ay, bx, by, width):
    half = width * 0.5
    stamp(pixels, size, 0, 0, lambda x, y: dist_segment(x, y, ax, ay, bx, by) - half)


def fill_circle(pixels, size, cx, cy, radius):
    stamp(pixels, size, 0, 0, lambda x, y: math.hypot(x - cx, y - cy) - radius)


def stroke_circle(pixels, size, cx, cy, radius, width):
    half = width * 0.5
    stamp(pixels, size, 0, 0, lambda x, y: abs(math.hypot(x - cx, y - cy) - radius) - half)


def fill_round_rect(pixels, size, x0, y0, x1, y1, radius):
    def d(x, y):
        cx = min(max(x, x0 + radius), x1 - radius)
        cy = min(max(y, y0 + radius), y1 - radius)
        if x0 + radius <= x <= x1 - radius or y0 + radius <= y <= y1 - radius:
            dx = 0 if x0 <= x <= x1 else min(abs(x - x0), abs(x - x1))
            dy = 0 if y0 <= y <= y1 else min(abs(y - y0), abs(y - y1))
            return math.hypot(dx, dy)
        return math.hypot(x - cx, y - cy) - radius

    stamp(pixels, size, 0, 0, d)


def icon(size=64):
    return bytearray(size * size * 4)


def save_icon(name, draw, size=64):
    pixels = icon(size)
    draw(pixels, size)
    write_png(os.path.join(ROOT, "Icons", name), size, size, pixels)


def rounded(size=48, radius=12):
    pixels = icon(size)
    for y in range(size):
        for x in range(size):
            dx = max(radius - (x + 0.5), 0.0, (x + 0.5) - (size - radius))
            dy = max(radius - (y + 0.5), 0.0, (y + 0.5) - (size - radius))
            alpha = clamp01(radius - math.hypot(dx, dy) + 0.5)
            i = (y * size + x) * 4
            pixels[i] = 255
            pixels[i + 1] = 255
            pixels[i + 2] = 255
            pixels[i + 3] = int(alpha * 255)
    write_png(os.path.join(ROOT, "Rounded12.png"), size, size, pixels)


def main():
    rounded()

    def close(p, s):
        stroke_segment(p, s, 18, 18, 46, 46, 5)
        stroke_segment(p, s, 46, 18, 18, 46, 5)

    def check(p, s):
        stroke_segment(p, s, 14, 34, 26, 46, 5.5)
        stroke_segment(p, s, 26, 46, 50, 18, 5.5)

    def lock(p, s):
        fill_round_rect(p, s, 18, 30, 46, 52, 4)
        stroke_circle(p, s, 32, 26, 10, 4.5)

    def plus(p, s):
        stroke_segment(p, s, 32, 16, 32, 48, 5)
        stroke_segment(p, s, 16, 32, 48, 32, 5)

    def back(p, s):
        stroke_segment(p, s, 38, 16, 18, 32, 5)
        stroke_segment(p, s, 18, 32, 38, 48, 5)

    def send(p, s):
        stroke_segment(p, s, 16, 18, 48, 32, 4.5)
        stroke_segment(p, s, 48, 32, 16, 46, 4.5)
        stroke_segment(p, s, 16, 18, 16, 46, 4.5)
        stroke_segment(p, s, 16, 32, 48, 32, 3.5)

    def members(p, s):
        fill_circle(p, s, 24, 22, 8)
        fill_circle(p, s, 42, 24, 7)
        fill_circle(p, s, 24, 46, 12)
        fill_circle(p, s, 44, 48, 10)

    def mute(p, s):
        fill_round_rect(p, s, 14, 24, 26, 40, 2)
        stroke_segment(p, s, 26, 24, 40, 16, 4)
        stroke_segment(p, s, 26, 40, 40, 48, 4)
        stroke_segment(p, s, 40, 16, 40, 48, 4)
        stroke_segment(p, s, 18, 48, 48, 16, 4)

    def kick(p, s):
        fill_circle(p, s, 26, 20, 7)
        fill_round_rect(p, s, 14, 32, 38, 52, 8)
        stroke_segment(p, s, 40, 36, 52, 48, 4.5)
        stroke_segment(p, s, 52, 36, 40, 48, 4.5)

    def circle(p, s):
        fill_circle(p, s, s * 0.5, s * 0.5, s * 0.46)

    save_icon("IconClose.png", close)
    save_icon("IconCheck.png", check)
    save_icon("IconLock.png", lock)
    save_icon("IconPlus.png", plus)
    save_icon("IconBack.png", back)
    save_icon("IconSend.png", send)
    save_icon("IconMembers.png", members)
    save_icon("IconMute.png", mute)
    save_icon("IconKick.png", kick)
    save_icon("Circle.png", circle)


if __name__ == "__main__":
    main()
