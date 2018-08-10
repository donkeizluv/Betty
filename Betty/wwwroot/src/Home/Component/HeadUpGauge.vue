<template>
    <div style="display: inline-flex; width: 100%; height: 100%; text-align: center">
        <div class="f1" :style="{width: f1Width}">{{f1Text}}</div>
        <div class="f2" :style="{width: f2Width}">{{f2Text}}</div>
    </div>
</template>
<script>
export default {
    name: "HeadUpGauge",
    created() {
        this.f1Animate = this.f1;
        this.f2Animate = this.f2;
    },
    props: {
        f1: {
            type: Number,
            required: true
        },
        f2: {
            type: Number,
            required: true
        },
        suffix: {
            type: String,
            default: "%"
        }
    },
    computed: {
        f1Width() {
            return `${this.truncate(this.f1Animate)}%`;
        },
        f2Width() {
            return `${this.truncate(this.f2Animate)}%`;
        },
        f1Text() {
            //No text if too small
            if (this.isZero) return `0${this.suffix}`;
            if (this.f1 <= 10) return "";
            return `${this.f1}${this.suffix}`;
        },
        f2Text() {
            if (this.isZero) return `0${this.suffix}`;
            if (this.f2 <= 10) return "";
            return `${this.f2}${this.suffix}`;
        },
        isZero() {
            return this.f1 == 0 && this.f2 == 0;
        }
    },
    watch: {
        f1(newVal, oldVal) {
            let vm = this;
            let dif = newVal - oldVal;
            let minus = dif <= 0;
            let frame = 0;
            //30FPS
            let animation = setInterval(() => {
                if (frame >= Math.abs(dif)) {
                    //Stop animation
                    clearInterval(animation);
                    return;
                }
                if (!minus) vm.f1Animate += 1;
                else vm.f1Animate -= 1;
                frame++;
            }, 33);
        },
        f2(newVal, oldVal) {
            let vm = this;
            let dif = newVal - oldVal;
            let minus = dif <= 0;
            let frame = 0;
            let animation = setInterval(() => {
                if (frame >= Math.abs(dif)) {
                    clearInterval(animation);
                    return;
                }
                if (!minus) vm.f2Animate += 1;
                else vm.f2Animate -= 1;
                frame++;
            }, 33);
        }
    },
    data() {
        return {
            f1Animate: 0,
            f2Animate: 0
        };
    },
    methods: {
        truncate(w) {
            if (this.isZero) return 50;
            if (w >= 94) return 94;
            if (w <= 6) return 6;
            return w;
        }
    }
};
</script>
<style scoped>
/* .text-shadow {
    text-shadow: -1px 0 black, 0 1px black, 1px 0 black, 0 -1px black;
} */
.f1 {
    color: #fafafa;
    background-color: #c62828;
    border-radius: 150px 0px 0px 20px;
}
.f2 {
    color: #fafafa;
    background-color: #212121;
    border-radius: 0px 20px 150px 0px;
}
</style>