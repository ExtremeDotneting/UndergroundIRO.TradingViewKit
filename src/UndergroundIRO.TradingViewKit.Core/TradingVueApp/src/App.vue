<template>
    <!-- TradingVueJs 101 (example from 'Getting Started' ) -->

    <trading-vue class="trading_view"
                 :color-back="colors.colorBack"
                 :color-grid="colors.colorGrid"
                 :color-text="colors.colorText"
                 ref="tradingVue"
                 :data="chart"
                 :width="this.width"
                 :height="this.height"></trading-vue>
</template>

<style lang="css">
    @import "main.css";
</style>

<script>
    import TradingVue from "trading-vue-js";
    import Data from "../data/data.json";
    import { constants } from "crypto";

    export default {
        name: "app",
        components: { TradingVue },
        methods: {
            onResize(event) {
                this.width = window.innerWidth;
                this.height = window.innerHeight - 5;
            }
        },
        mounted() {
            window.addEventListener("resize", this.onResize);
            window["TradingVueObj"] = this.$refs.tradingVue;
            this.$refs.tradingVue.titleTxt = "";
            this.onResize();
            setInterval(function () {
                var tv = window["TradingVueObj"];
                var ctx = window["TradingViewContext"];
                var range = tv.getRange();               
                if (range[0] != ctx.currentRangeStart || range[1] != ctx.currentRangeEnd) {
                    ctx.currentRangeStart = range[0];
                    ctx.currentRangeEnd = range[1];
                    if(window.timeRangeUpdated)
                        window.timeRangeUpdated(ctx.currentRangeStart, ctx.currentRangeEnd);
                }
            }, 50);
        },
        beforeDestroy() {
            window.removeEventListener("resize", this.onResize);
        },
        data() {
            window["TradingViewContext"] = {
                currentRangeStart: 0,
                currentRangeEnd: 0,
                chart: Data,
                width: window.innerWidth,
                height: window.innerHeight,
                colors: {
                    colorBack: '#fff',
                    colorGrid: '#eee',
                    colorText: '#333',
                }
            };
            return window["TradingViewContext"];
        }
    };
</script>
